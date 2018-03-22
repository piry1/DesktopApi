var app = angular.module("myApp", []);

app.controller("myCtrl", function ($scope, $http, $interval) {

    var startAnimation = "bounceInDown";
    var port = 5000;
    const url = 'ws://localhost:' + port + '/';
    var socket;
    var start = true; // start of application
    var activeCategory = "";

    $scope.show = false;
    $scope.CenterHelpText = "";

    const socketMessageListener = (event) => {
        var receivedMsg = event.data;
        var obj = JSON.parse(receivedMsg);
        $scope.switchControllerResponse(obj);
    };

    const socketOpenListener = () => {
        console.log('Connected');
        sendSocketRequest("categories", "get", []);
    };

    const socketErrorListener = () => {
        // socket.onclose = function () { };
        if (socket) {
            socket.removeEventListener('close', socketErrorListener);
            socket.close();
        }
        socket = new WebSocket(url);
        socket.addEventListener('open', socketOpenListener);
        socket.addEventListener('message', socketMessageListener);
        socket.addEventListener('error', socketErrorListener);
        socket.addEventListener('close', socketErrorListener);
    };

    window.onbeforeunload = () => {
        if (socket) {
            socket.removeEventListener('close', socketErrorListener);
            socket.close();
        }
    };

    socketErrorListener();

    function sendSocketRequest(controller, method, params) {
        var socketRequest = { Controller: controller, Method: method, Params: params };
        socket.send(JSON.stringify(socketRequest));
    }

    $scope.switchControllerResponse = function (data) {
        switch (data.Controller) {
            case "categories":
                $scope.categoriesResponse(data);
                break;
            case "desktop":
                $scope.desktopResponse(data);
                break;
            case "notification":
                $scope.notificationResponse(data);
                break;
        }
    };

    $scope.desktopResponse = function (data) {
        switch (data.Method) {
            case "get":
                $scope.elems = data.Response;
                $scope.$apply();
                break;
            default:
        }

    }

    $scope.notificationResponse = function (data) {
        switch (data.Method) {
            case "changed":
                console.log("changed");
                $scope.GetCateegory(null, activeCategory);
                break;
            default:
        }

    };

    $scope.categoriesResponse = function (data) {
        switch (data.Method) {
            case "get":
                $scope.categories = data.Response;
                $scope.$apply();
                $scope.GetCateegory(null, data.Response[0]);
                dropableMenu();
                break;
        }

    };

    // Get all categories names
    $scope.GetCateegories = function () {
        sendSocketRequest("categories", "get", []);
    };

    $scope.ChangeCategory = function (id, value) {
        sendSocketRequest("categories", "renamebyid", [id, value]);
        console.log("change category");
    };

    $scope.GetCateegory = function ($event, catName) {
        sendSocketRequest("desktop", "get", [catName]);
        activeCategory = catName;
        // make category btn active
        if ($event !== null && $event !== undefined)
            toggleNavButtons($event.currentTarget);
    };

    // Animate icons on actions
    $scope.AnimateIcon = function ($event, enable) {
        var icon = $event.currentTarget;
        var animationName = "swing";
        var btn;

        if (enable) {
            $(icon).removeClass(startAnimation);
            $(icon).addClass(animationName);
            btn = $(icon).children()[1];
            $(btn).removeClass("hidden");
        } else {
            btn = $(icon).children()[1];
            $(btn).addClass("hidden");
            $(icon).removeClass(animationName);
        }
    };

    // Show Context menu
    $scope.ContextMenu = function (e, id) {
        sendSocketRequest("file", "openmenu", [id]);
    };

    // closing context menu
    $scope.CloseContentMenu = function () {
        sendSocketRequest("file", "closemenu", []);
    };

    // Open file or directory
    $scope.Start = function ($event, id) {
        $scope.CloseContentMenu();
        if ($scope.clickInterval < 50)
            sendSocketRequest("file", "start", [id]);
        $('.icon-div').removeClass("active");
        $($event.currentTarget).addClass("active");
        $scope.clickInterval = 0;
    };

    // show or hide icons
    $scope.ToggleApp = function () {
        $scope.show = !$scope.show;
        if ($scope.show === true)
            $('.icon-div').addClass(startAnimation);
        if (start) { // makes first category active
            start = false;
            $($('.btn-auto')[0]).addClass('active');
        }
    };

    $scope.IconMouseEnter = function ($event, isOn) {
        $scope.AnimateIcon($event, isOn);
    };

    // function that run when background is clicked
    $scope.BackgroundClick = function () {
        $scope.CloseContentMenu();
        $('.icon-div').removeClass('active');
    };

    $interval(function () { $scope.clickInterval++; }, 5);

    function dropableMenu() {
        $(".category").droppable({
            drop: function (event, ui) {
                $scope.ChangeCategory(ui.draggable[0].id, $(this).attr("id"));
                ui.draggable.remove();
            }
        });
    }
});


// JQUERY CODE

function toggleNavButtons(elem) {
    $('button').removeClass("active");
    $(elem).addClass("active");
}

$(function () {
    $("#context").sortable({
        handle: 'img',
        cancel: ''
    });
    $("#context").disableSelection();
});
