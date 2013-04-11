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
            feedCategoryTemplate = '<div class="accordion-group"><div class="accordion-heading"><a class="accordion-toggle" data-toggle="collapse" data-parent="#feed-category-list" href="#{Id}">{Name}</a></div><div id="{Id}" class="accordion-body collapse"><div class="accordion-inner"><ul class="nav nav-pills nav-stacked"></ul></div></div></div>',
            feedTemplate = '<li><a href="#">{Name} <span class="badge badge-info">{UnreadCount}</span></a></li>';

        function getFeeds() {
            feedHub.server.getFeeds().done(function (feeds) {
                $.each(feeds, function () {
                    var feed = this,
                        $category = $('#' + feed.Category + ' .accordion-inner ul', $categoryList);
                    if ($category.length == 0) {
                        var category = {
                            Id: feed.Category,
                            Name: feed.Category
                        };
                        $categoryList.append(feedCategoryTemplate.supplant(category));
                        $category = $('#' + feed.Category + ' .accordion-inner ul', $categoryList);
                    }
                    $category.append(feedTemplate.supplant(feed));
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
                getFeeds();
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
