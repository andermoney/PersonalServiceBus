﻿// Crockford's supplant method (poor man's templating)
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

$(function () {
    var feedHub = $.connection.feedHub,
        $categoryList = $('#feed-category-list'),
        feedCategoryTemplate = '<div class="accordion-group"><div class="accordion-heading"><a class="accordion-toggle" data-toggle="collapse" data-parent="#feed-category-list" href="#{CategoryId}">{CategoryName}</a></div><div id="{CategoryId}" class="accordion-body collapse"><div class="accordion-inner">Feeds here...</div></div></div>';

    function formatCategory(category) {
        return $.extend(category, {

        });
    }

    function init() {
        return feedHub.server.getFeedCategories().done(function (categories) {
            $.each(categories, function () {
                var category = formatCategory(this);
                $categoryList.append(feedCategoryTemplate.supplant(category));
            });
        });
    }

    //client methods the server will call back
    $.extend(feedHub.client, {
        updateCategoryFeeds: function (category) {
            var $category = $('#' + category.CategoryId + ' .accordion-inner');
            $category.html('Feed count updated.');
        }
    });

    //start the connection
    $.connection.hub.start()
        .pipe(init)
        .done(function () {
        });
});