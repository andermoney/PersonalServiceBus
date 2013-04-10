define(['/Scripts/jquery.signalR-1.0.1.min.js', '/signalr/hubs'], function (signalR, hubs) {
    // Crockford's supplant method (poor man's templating)
    if (!String.prototype.supplant) {
        String.prototype.supplant = function (o) {
            return this.replace(/{([^{}]*)}/g,
                function (a, b) {
                    var r = o[b];
                    return typeof r === 'string' || typeof r === 'number' ? r : a;
                }
            );
        };
    }

    function createHub() {
        var feedHub = $.connection.feedHub,
            $categoryList = $('#feed-category-list'),
            feedCategoryTemplate = '<div class="accordion-group"><div class="accordion-heading"><a class="accordion-toggle" data-toggle="collapse" data-parent="#feed-category-list" href="#{CategoryId}">{CategoryName}</a></div><div id="{CategoryId}" class="accordion-body collapse"><div class="accordion-inner">Feeds here...</div></div></div>';

        function formatCategory(category) {
            return $.extend(category, {

            });
        }

        function getFeedCategories() {
            feedHub.server.getFeedCategories().done(function (categories) {
                $.each(categories, function () {
                    var category = formatCategory(this);
                    $categoryList.append(feedCategoryTemplate.supplant(category));
                });
            });
        }

        function setupClient() {
            //client methods the server will call back
            $.extend(feedHub.client, {
                updateCategoryFeeds: function (category) {
                    var $category = $('#' + category.CategoryId + ' .accordion-inner');
                    $category.html('Feed count updated.');
                }
            });
        }

        //start the connection
        $.connection.hub.start({ waitForPageLoad: false })
            .done(function () {
                setupClient();
                $(document).trigger('hubStarted');
                getFeedCategories();
            });
        return feedHub;
    }

    function showError(status, parentElement) {
        var hubWarningTemplate = '<div class="alert"><button type="button" class="close" data-dismiss="alert">&times;</button><strong>Warning!</strong>&nbsp;<span class="alert-message">{ErrorMessage}</span></div>',
            hubErrorTemplate = '<div class="alert alert-error"><button type="button" class="close" data-dismiss="alert">&times;</button><strong>Error!</strong>&nbsp;<span class="alert-error-message">{ErrorMessage}</span></div>',
            errorContainer;
        if ($(parentElement).length > 0) {
            errorContainer = $('.alert-container', parentElement);
        } else {
            errorContainer = $('.alert-container');
        }

        if (status.ErrorLevel > 3) {
            errorContainer.append(hubErrorTemplate.supplant(status));
        } else if (status.ErrorLevel == 3) {
            errorContainer.append(hubWarningTemplate.supplant(status));
        }
    }

    return {
        hub: createHub(),
        showError: showError
    };
});
