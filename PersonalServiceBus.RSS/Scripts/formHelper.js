define([], function () {
    function getFormData($elem) {
        var data = {};
        $('input', $elem).each(function () {
            var name = $(this).attr('name');
            if (name) {
                data[name] = $(this).val();
            }
        });
        return data;
    }
    
    function clearForm($elem) {
        $('input', $elem).each(function () {
            var name = $(this).attr('name');
            if (name) {
                $(this).val('');
            }
        });
    }

    return {
        getFormData: getFormData,
        clearForm: clearForm
    };
});