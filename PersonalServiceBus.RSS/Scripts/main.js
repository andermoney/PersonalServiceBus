﻿requirejs([], function() {
    require.config({
        paths: {
            jquery: '/Scripts/jquery-1.9.1.min'
        },
        shims: {
            "/Scripts/jquery.signalR-1.0.1.min": {
                deps: ['jquery'],
                exports: '$.connection'
            },
            "/signalr/hubs": {
                deps: ['jquery', 'jquery.signalR-1.0.1.min']
            }
        }
    });
});

require(['/Scripts/jquery.signalR-1.0.1.min.js', '/signalr/hubs'], function(signalR, hubs) {
});