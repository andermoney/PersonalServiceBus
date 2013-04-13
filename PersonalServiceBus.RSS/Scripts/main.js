requirejs([], function() {
    require.config({
        paths: {
            jquery: '/Scripts/jquery-1.9.1.min.js'
        },
        shims: {
            "/Scripts/jquery.signalR-1.0.1.min.js": {
                deps: ['jquery'],
                exports: '$.connection'
            },
            "/signalr/hubs": {
                deps: ['jquery', '/Scripts/jquery.signalR-1.0.1.min.js']
            }
        }
    });
});

require(['/Scripts/jquery.signalR-1.0.1.min.js', '/signalr/hubs'], function(signalR, hubs) {
});