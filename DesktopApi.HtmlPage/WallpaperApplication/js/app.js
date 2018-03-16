var app = angular.module("myApp", []);

app.controller("myCtrl", function ($scope, $http, $interval) {

    var startAnimation = 'bounceInDown';
    var port = 5001;
    var apiUrl = 'http://localhost:' + port + '/';
    var start = true; // start of application
    var lastRequestUrl = "";
    var desktopId = Math.random();

    $scope.show = false;
    $scope.preventBgClick = false; // prevent from runing BackgroundClick method
    $scope.clickInterval = 0;
    $scope.CenterHelpText = "";

    // Get all categories names
    $scope.GetCateegories = function() {
        $http.get(apiUrl + 'categories/get')
            .then(function(response) {
                $scope.categories = response.data;
                console.log($scope.categories);
                dropableMenu();
            });
    };

    $scope.ChangeCategory = function(id, value) {
        $http.get(apiUrl + 'categories/renamebyid/' + id + '/' + value)
            .then(function(response) {
                console.log(response.data);
            });
    };

    // API request
    $scope.GetElems = function(url) {
        $http.get(url)
            .then(function(response) {
                $scope.elems = response.data;
                console.log($scope.elems);
            });
    };

    // GET category from API
    $scope.GetCateegory = function($event, catName) {
        lastRequestUrl = apiUrl + 'desktop/get/' + catName;
        $scope.GetElems(lastRequestUrl);

        if ($event !== null)
            toggleNavButtons($event.currentTarget);
    };

    // Animate elems icons on actions
    $scope.AnimateIcon = function($event, enable) {
        var icon = $event.currentTarget;
        var animationName = 'swing';
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
    $scope.ContextMenu = function(e, id) {
        $http.get(apiUrl + 'file/openmenu/' + id);
    };

    // closing context menu
    $scope.CloseContentMenu = function() {
        $http.get(apiUrl + 'file/closemenu');
        console.log("context menu closing");
        // sortable();
    };

    // Open file or directory
    $scope.Start = function($event, id) {
        $scope.CloseContentMenu();
        if (!$scope.preventStart && $scope.clickInterval < 50)
            $http.get(apiUrl + 'file/start/' + id);
        $('.icon-div').removeClass("active");
        $($event.currentTarget).addClass("active");
        $scope.clickInterval = 0;
    };

    // bla bla bla
    $scope.PreventBgClick = function(p) {
        $scope.preventBgClick = p;
    };

    // show or hide icons
    $scope.ToggleApp = function() {
        $scope.show = !$scope.show;
        if ($scope.show === true)
            $('.icon-div').addClass(startAnimation);
        if (start) {
            start = false;
            // console.log();
            $($('.btn-auto')[0]).addClass('active');
        }
    };

    // check if theare are any changes on desktop and load data if theare are
    $scope.Changed = function() {
        $http.get(apiUrl + 'desktop/changed/' + desktopId)
            .then(function(response) {
                console.log(response.data);
                if (response.data === "true") {
                    $scope.GetCateegories();
                    $scope.GetElems(lastRequestUrl);
                }
            });
    };

    $scope.IconMouseEnter = function($event, isOn) {
        if (isOn) {
            //  console.log($event.currentTarget);
            var a = $event.currentTarget;
            var b = $(a).find('span')[1].innerText;
            // console.log(b);
            $scope.CenterHelpText = b;
        } else
            $scope.CenterHelpText = "";

        $scope.AnimateIcon($event, isOn);
        $scope.PreventBgClick(isOn);
    };

    // function that run when background is clicked
    $scope.BackgroundClick = function() {
        if (!$scope.preventBgClick) {
            console.log("bg-click");
            $scope.CloseContentMenu();
            $('.icon-div').removeClass('active');
        }

    };

    $scope.HideIcons = function(param) {
        $http.get(apiUrl + 'desktop/icons/' + param);
    };

    // Start at aplication start and load all data
    $scope.OnStart = function() {
        // get categories
        $http.get(apiUrl + 'categories/get')
            .then(function(response) {
                $scope.categories = response.data;
                // get first category files
                $scope.GetCateegory(null, response.data[0]);
            });

        $interval($scope.Changed, 1000); // check changes
        $interval(function() { $scope.clickInterval++; }, 1); // for double click
    };

    $scope.OnStart();
    function dropableMenu() {
        $(".category").droppable({
            drop: function (event, ui) {
                // console.log($(this).attr("id"));
                //  console.log(ui.draggable[0].id);
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



// jQuery(function($){
// 	$( document )
// 		.drag("start",function( ev, dd ){
// 			return $('<div class="selection" />')
// 				.css('opacity', .65 )
// 				.appendTo( document.body );
// 		})
// 		.drag(function( ev, dd ){
// 			$( dd.proxy ).css({
// 				top: Math.min( ev.pageY, dd.startY ),
// 				left: Math.min( ev.pageX, dd.startX ),
// 				height: Math.abs( ev.pageY - dd.startY ),
// 				width: Math.abs( ev.pageX - dd.startX )
// 			});
// 		})
// 		.drag("end",function( ev, dd ){
// 			$( dd.proxy ).remove();
// 		});
// 	$('.icon-div')
// 		.drop("start",function(){
// 			$( this ).addClass("active");
// 		})
// 		.drop(function( ev, dd ){
// 			$( this ).toggleClass("dropped");
// 		})
// 		.drop("end",function(){
// 			$( this ).removeClass("active");
// 		});
// 	$.drop({ multi: true });	
// });

