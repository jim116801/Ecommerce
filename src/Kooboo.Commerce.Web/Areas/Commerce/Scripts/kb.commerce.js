﻿(function ($) {

    var kb = window.kb = window.kb || {};

    kb.registerNamespace = function (ns) {
        var parts = ns.split('.');
        var parent = window;
        $.each(parts, function () {
            parent[this] = parent[this] || {};
            parent = parent[this];
        })

        return parent;
    };

    (function () {
        var handlersByEventName = {};

        kb.events = {
            on: function (eventName, handler) {
                var handlers = handlersByEventName[eventName];
                if (!handlers) {
                    handlers = [];
                    handlersByEventName[eventName] = handlers;
                }

                handlers.push(handler);
            },
            trigger: function (eventName, sender, args) {
                var deferred = $.Deferred();
                var handlers = handlersByEventName[eventName];
                if (handlers) {
                    var promises = [];

                    $.each(handlers, function () {
                        var result = this(sender, args);
                        if (result && result.then && typeof(result.then) === 'function') {
                            promises.push(result);
                        }
                    });

                    if (promises.length > 0) {
                        $.when.apply($, promises)
                         .then(function () {
                             deferred.resolve();
                         });
                    } else {
                        deferred.resolve();
                    }
                } else {
                    deferred.resolve();
                }

                return deferred.promise();
            }
        };
    })();

    (function () {
        kb.http = {
            safeGet: function (url, data) {
                return $.get(url, data)
                        .fail(function (xhr) {
                            var result = JSON.parse(xhr.responseText);
                            showError(result);
                        });
            },
            safePost: function (url, data) {
                return $.post(url, data)
                        .fail(function (xhr) {
                            var result = JSON.parse(xhr.responseText);
                            showError(result);
                        });
            }
        };

        function showError(error) {
            window.loading.hide();
            info.show(error.message, false);
        }
    })();

})(jQuery);