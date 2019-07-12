app.controller('AppController', function ($scope, AppService)
{
	var maxTimestamp;
	$scope.filtersName = ["logger","level","timestamp", "thread", "message"]
	$scope.selectedNames = ["logger","level","timestamp"]
	$scope.threadRanges = ["empty","<=10","10-40","40-80","80-110", ">110"]

	$scope.WaitingInfo = true;
	var service = AppService.getLogs();
	service.then(function (d)  
        {  
            $scope.logsData = d.data; 
			$scope.$applyAsync(); 
			maxTimestamp = $scope.logsData[0].timestamp;
			setInterval(refreshLogs,2000);
        }, function (error)  
        {  
        }) 

	$scope.sort = function(selectedName){
		var service = AppService.getSortedLogs();
		service.then(function (d)  
        {  
            $scope.logsData = d.data;  
        }, function (error)  
        {  
        }) 

		$scope.$applyAsync();
	}

	$scope.filter = function(){
		var filteredData = { text: $scope.filterText, column: $scope.filterColumn, threadRange: $scope.threadColumn};
		AppService.filterLogs(filteredData);
		readFiltredLogs();
	}

	function readFiltredLogs(){
		$scope.logsData = AppService.getFilteredLogs();   
		$scope.$applyAsync();
	}

	$scope.filterAngular = function(){
		return function(item){
		if($scope.filterColumn === "level"){
			return item.level.includes($scope.filterText.toUpperCase()) || item.level.includes($scope.filterText.toLowerCase()); 
		}else if($scope.filterColumn === "thread"){
			return item.thread === $scope.filterText;
		}else if($scope.filterColumn === "logger"){
			return item.logger.includes($scope.filterText.toUpperCase()) || item.logger.includes($scope.filterText.toLowerCase());
		}else if($scope.filterColumn === "message"){
			return item.message.includes($scope.filterText.toUpperCase()) || item.message.includes($scope.filterText.toLowerCase());
		}else if($scope.threadColumn === 'empty' && ($scope.filterColumn === "" || typeof $scope.filterColumn === 'undefined')){
			return item;
		}
		if($scope.threadColumn !== 'empty'){
			if($scope.threadColumn === "<=10"){
				return item.thread <= 10;
			}else if($scope.threadColumn === "10-40"){
				return item.thread >10 && item.thread <= 40;
			}else if($scope.threadColumn === "40-80"){
				return item.thread > 40 && item.thred <= 80;
			}else if($scope.threadColumn === "80-110"){
				return item.thread >80 && item.thread <=110;
			}else if($scope.threadColumn === ">110"){
				return item.thread > 110;
			}
		}
		}
	}

	function refreshLogs(){
		$scope.logsData.forEach(function(item){
			if(item.timestamp > maxTimestamp){
				maxTimestamp = item.timestamp;
			}});

		var service = AppService.getRefreshedLogs(maxTimestamp);
		service.then(function (d)  
        {  
			var freshedLogs = d.data.concat($scope.logsData);
			//$scope.logsData = $scope.logsData.concat(d.data);
			$scope.logsData = freshedLogs;
        }, function (error)  
        {  
        }) 
	}
})