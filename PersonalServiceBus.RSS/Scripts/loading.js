define(['/Scripts/mustache.js'], function (mustache) {
    var loadingTemplate = '<img src="/Content/img/loading.gif" alt="loading..." class="icon-loading" />';
    
    function addLoadingIcon($elem) {
        $elem.append(loadingTemplate);
    }
    
    function removeLoadingIcon($elem) {
        $('.icon-loading', $elem).remove();
    }

    return {
        addLoadingIcon: addLoadingIcon,
        removeLoadingIcon: removeLoadingIcon
    };
});