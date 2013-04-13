﻿define(['/Scripts/jquery.signalR-1.0.1.min.js', '/signalr/hubs'], function (signalR, hubs) {
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

    // A simple background color flash effect that uses jQuery Color plugin
    $.fn.flash = function (color, duration) {
        var current = this.css('backgroundColor');
        this.animate({ backgroundColor: 'rgb(' + color + ')' }, duration / 2)
            .animate({ backgroundColor: current }, duration / 2)
            .animate({ backgroundColor: 'rgb(' + color + ')' }, duration / 2)
            .animate({ backgroundColor: current }, duration / 2);
    };

    function formatFeed(feed) {
        return $.extend(feed, {
            Id: feed.Id.replace('/', '-')
        });
    }

    function createHub() {
        var feedHub = $.connection.feedHub;

        function getFeeds() {
            feedHub.server.getFeeds().done(function (feedResponse) {
                if (feedResponse.Status.ErrorLevel > 2) {
                    showError(feedResponse.Status);
                }
                $.each(feedResponse.Data, function () {
                    var feed = formatFeed(this);
                    addFeed(feed);
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
        var hubWarningTemplate = '<div class="alert fade in"><button type="button" class="close" data-dismiss="alert">&times;</button><strong>Warning!</strong>&nbsp;<span class="alert-message">{ErrorMessage}</span></div>',
            hubErrorTemplate = '<div class="alert alert-error fade in"><button type="button" class="close" data-dismiss="alert">&times;</button><strong>Error!</strong>&nbsp;<span class="alert-error-message">{ErrorMessage}</span></div>',
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

    function addFeedAnimation($categoryList, category, $feed) {
        var $shownCategory = $('.in', $categoryList),
            $category = $('#' + category, $categoryList);
        if ($category.attr('id') != $shownCategory.attr('id')) {
            $shownCategory.collapse('hide');
            $category.collapse('show');
        }
        $feed.flash('255, 248, 86', 1000);
    }

    function addFeed(feed, showAnimation) {
        var $categoryList = $('#feed-category-list'),
            feedCategoryTemplate = '<div class="accordion-group"><div class="accordion-heading"><a class="accordion-toggle" data-toggle="collapse" data-parent="#feed-category-list" href="#{Id}">{Name}</a></div><div id="{Id}" class="accordion-body collapse"><div class="accordion-inner"><ul class="nav nav-pills nav-stacked"></ul></div></div></div>',
            feedTemplate = '<li id="{Id}"><a href="#">{Name} <span class="badge badge-info">{UnreadCount}</span></a></li>',
            $category = $('#' + feed.Category + ' .accordion-inner ul', $categoryList),
            $feed;

        feed = formatFeed(feed);

        if ($category.length == 0) {
            var category = {
                Id: feed.Category,
                Name: feed.Category
            };
            $categoryList.append(feedCategoryTemplate.supplant(category));
            $category = $('#' + feed.Category + ' .accordion-inner ul', $categoryList);
        }
        $feed = $('#' + feed.Id, $category);
        if ($feed.length == 0) {
            $category.append(feedTemplate.supplant(feed));
            $feed = $('#' + feed.Id, $category);
        }
        if (showAnimation == true) {
            addFeedAnimation($categoryList, feed.Category, $feed);
        }
    }

    return {
        hub: createHub(),
        showError: showError,
        addFeed: addFeed
    };
});
