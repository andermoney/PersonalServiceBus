(function() {
    angular.module('FeedModule')
        .directive('categoryDirective', function() {
            return {
                restrict: 'E',
                templateUrl: 'views/feedCategoryView.html',
                scope: {
                    category: "=data"
                }
            }
        });
})();