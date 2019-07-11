app.controller('AppController', function ($scope, AppService)
{
	var maxTimestamp;
	$scope.filtersName = ["logger","level","timestamp", "thread"]
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

	function refreshLogs(){
		var maxTimestamp =0;
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