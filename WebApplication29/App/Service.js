app.service("AppService", function ($http)
{
    let filteredLogs; 
	this.getLogs = function ()
	{
		return $http.get("http://localhost:17879/api/Logs");
	}

	this.getRefreshedLogs = function (maxTimestamp)
	{
		return $http.get("http://localhost:17879/api/RefreshedLogs/"+ maxTimestamp);
	}

	this.getSortedLogs = function ()
	{
		return $http.get("http://localhost:17879/api/SortedLogs");
	}

	this.filterLogs = function (filterData)
	{
		$http({
			method : "POST",
			url : "http://localhost:17879/api/FilterLogs",
			data : JSON.stringify(filterData),
			headers : {
				'Content-Type' : 'application/json'
			},
		}).then(function(response) { 
			filteredLogs = response.data; 
		});
	}

	this.getFilteredLogs = function ()
	{
		return filteredLogs;
	}
});