﻿define(['/Scripts/jquery.signalR-1.0.1.min.js', '/signalr/hubs', '/Scripts/template.js', '/Scripts/loading.js' , '/Scripts/notificationHelper.js'], function (signalR, hubs, template, loading, notificationHelper) {
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

    function formatUserFeed(userFeed) {
        return $.extend(userFeed, {
            Id: userFeed.Id.replace(/\//g, '-'),
            Feed: $.extend(userFeed.Feed, {
                Id: userFeed.Feed.Id.replace(/\//g, '-')
            })
        });
    }
    
    function formatFeedItem(userFeedItem) {
        return $.extend(userFeedItem, {
            Id: userFeedItem.Id.replace(/\//g, '-')
        });
    }

    function formatFeedCategory(category) {
        return $.extend(category, {            
            Id: category.Id.replace(/\//g, '-')
                .replace(/ /g, '-')
        });
    }
    
    function formatUserFeedItem(feedItem) {
        return $.extend(feedItem, {            
           Id: feedItem.Id.replace(/\//g, '-') 
        });
    }

    function createHub() {
        var feedHub = $.connection.feedHub;

        function getFeeds() {
            loading.addLoadingIcon($categoryList);
            feedHub.server.getFeeds().done(function(feedResponse) {
                loading.removeLoadingIcon($categoryList);
                if (feedResponse.Status.ErrorLevel > 2) {
                    notificationHelper.showError(feedResponse.Status);
                } else {
                    $(document).trigger('feedListRetrieved');
                }
                $.each(feedResponse.Data, function() {
                    var feed = formatUserFeed(this);
                    addFeed(feed);
                });
            }).fail(function(error) {
                notificationHelper.showError({
                    ErrorLevel: 4,
                    ErrorMessage: 'Error getting feeds:' + error
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
            feedTemplate = '<li id="{Id}" class="feed"><a href="#">{Name} <span class="badge badge-important feed-error" style="display:none">!</span>&nbsp;<span class="badge badge-info badge-unread">{UnreadCount}</span></a></li>',
            $category = $('#' + feed.Category + ' .accordion-inner ul', $categoryList),
            $feed,
            feedId = feed.Id;

        feed = formatUserFeed(feed);

        if ($category.length == 0) {
            var category = formatFeedCategory({
                Id: feed.Category,
                Name: feed.Category
            });
            $categoryList.append(feedCategoryTemplate.supplant(category));
            $category = $('#' + category.Id + ' .accordion-inner ul', $categoryList);
        }
        $feed = $('#' + feed.Id, $category);
        if ($feed.length == 0) {
            $category.append(feedTemplate.supplant(feed));
            $feed = $('#' + feed.Id, $category);
            if (feed.Status && feed.Status.ErrorLevel > 2) {
                $('.feed-error', $feed).show();
            }
            $feed.click(function() {
                getFeedItems(feedId, $feed);
            });
        }
        if (showAnimation == true) {
            addFeedAnimation(feed.Category, $feed);
        }
    }
    
    function getFeedItems(feedId, $feed) {
        var feedHub = $.connection.feedHub,
            feedItems = [];

        loading.addLoadingIcon($feed);
        feedHub.server.getFeedItems({ Id: feedId }).done(function (getFeedItemsResponse) {
            loading.removeLoadingIcon($feed);
            if (getFeedItemsResponse.Status.ErrorLevel > 2) {
                notificationHelper.showError(getFeedItemsResponse.Status);
            } else {
                $(document).trigger('feedItemsRetrieved');
            }
            $.each(getFeedItemsResponse.Data, function() {
                var feedItem = formatUserFeedItem(this);
                feedItems.append(feedItem);
            });
            addFeedItems(feedItems);
        }).fail(function(error) {
            notificationHelper.showError({
                ErrorLevel: 4,
                ErrorMessage: 'Error getting feed items:' + error
            });
        });
    }
    
    function addFeedItems(feedItems) {
        var feedItemTemplate = '<div class="feed-item"><div class="feed-item-title"><span class="title">{Title}</span><span>{Author}</span></div><div class="feed-item-content">{Content}</div></div>',
            $feedView = $('.feed-view'),
            i,
            feedItem;
        for (i = 0; i < feedItems.length; i++) {
            feedItem = formatFeedItem(feedItems[i]);
            $feedView.append(feedItemTemplate.supplant(feedItem));
        }
    }

    return {
        hub: createHub(),
        addFeed: addFeed,
        addFeedItems: addFeedItems
    };
});
