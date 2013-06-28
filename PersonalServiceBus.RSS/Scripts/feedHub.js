define(['/Scripts/jquery.signalR-1.0.1.min.js', '/signalr/hubs', '/Scripts/template.js', '/Scripts/loading.js' , '/Scripts/notificationHelper.js'], function (signalR, hubs, template, loading, notificationHelper) {
    var $categoryList = $('#feed-category-list');

    // A simple background color flash effect that uses jQuery Color plugin
    $.fn.flash = function (color, duration) {
        var current = this.css('backgroundColor');
        this.animate({ backgroundColor: 'rgb(' + color + ')' }, duration / 2)
            .animate({ backgroundColor: current }, duration / 2)
            .animate({ backgroundColor: 'rgb(' + color + ')' }, duration / 2)
            .animate({ backgroundColor: current }, duration / 2)
            .animate({ backgroundColor: 'rgb(' + color + ')' }, duration / 2)
            .animate({ backgroundColor: current }, duration / 2);
    };

    function formatFeed(feed) {
        return $.extend(feed, {
            Id: feed.Id.replace('/', '-')
        });
    }
    
    function formatUserFeed(userFeed) {
        return $.extend(userFeed, {
            Id: userFeed.Id.replace('/', '-'),
            Feed: $.extend(userFeed.Feed, {
                Id: userFeed.Feed.Id.replace('/', '-')
            })
        });
    }

    function createHub() {
        var feedHub = $.connection.feedHub;

        function getFeeds() {
            loading.addLoadingIcon($categoryList);
            feedHub.server.getFeeds().done(function (feedResponse) {
                loading.removeLoadingIcon($categoryList);
                if (feedResponse.Status.ErrorLevel > 2) {
                    notificationHelper.showError(feedResponse.Status);
                } else {
                    $(document).trigger('feedListRetrieved');
                }
                $.each(feedResponse.Data, function () {
                    var feed = formatUserFeed(this);
                    addFeed(feed);
                });
            });
        }

        function setupClient() {
            //client methods the server will call back
            $.extend(feedHub.client, {
                UpdateFeedUnreadCount: function (userFeed) {
                    var $userFeed;
                    userFeed = formatUserFeed(userFeed);
                    $userFeed = $('#' + userFeed.Id, $categoryList);
                    
                    $('.badge-unread', $userFeed).html(userFeed.UnreadCount);
                }
            });
        }

        setupClient();

        //start the connection
        $.connection.hub.start({ waitForPageLoad: false })
            .done(function () {
                $(document).trigger('hubStarted');
                getFeeds();
            });
        return feedHub;
    }

    function addFeedAnimation(category, $feed) {
        var $shownCategory = $('.in', $categoryList),
            $category = $('#' + category, $categoryList);
        if ($category.attr('id') != $shownCategory.attr('id')) {
            $shownCategory.collapse('hide');
            $category.collapse('show');
        }
        $feed.flash('255, 248, 86', 500);
    }

    function addFeed(feed, showAnimation) {
        var feedCategoryTemplate = '<div class="accordion-group"><div class="accordion-heading"><a class="accordion-toggle" data-toggle="collapse" data-parent="#feed-category-list" href="#{Id}">{Name}</a></div><div id="{Id}" class="accordion-body collapse"><div class="accordion-inner"><ul class="nav nav-pills nav-stacked"></ul></div></div></div>',
            feedTemplate = '<li id="{Id}"><a href="#">{Name} <span class="badge badge-important feed-error" style="display:none">!</span>&nbsp;<span class="badge badge-info badge-unread">{UnreadCount}</span></a></li>',
            $category = $('#' + feed.Category + ' .accordion-inner ul', $categoryList),
            $feed;

        feed = formatUserFeed(feed);

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
            $feed = $('#' + feed.Feed.Id, $category);
            if (feed.Status && feed.Status.ErrorLevel > 2) {
                $('.feed-error', $feed).show();
            }
        }
        if (showAnimation == true) {
            addFeedAnimation(feed.Category, $feed);
        }
    }

    return {
        hub: createHub(),
        addFeed: addFeed
    };
});
