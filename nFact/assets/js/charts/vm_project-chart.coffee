$ -> 
	vm = new App.ProjectChart()
	vm.create()
			
class App.ProjectChart
	create: ->
		json = $('#dataModel').html()
		dataModel = jQuery.parseJSON(json)
		@spec = dataModel.spec
		urlData = "/" + @spec + "/chart?format=json"
		this.getData(urlData, this.render, this)
	
	getData: (url, callback, scope) ->
		$.ajax
			type: "GET",
			url: url,
			contentType: "application/json"
			dataType: "html",
			data: "",
			cache: false,
			success: (result) ->
				data = JSON.parse(result)
				callback(data, scope)
			,
			error: (XMLHttpRequest, textStatus, errorThrown) -> 
				alert('error occurred')

	render: (jsonData, scope) ->
		barData = []
		maxDays = 0
		$.each( jsonData.environmentCycle, ( index, environment ) ->	
			days = []
			$.each( environment.cycleTimes, ( index, cycleTime ) ->	
				days.push(cycleTime.days)
			)

			barData.push({name: environment.name, data: days})
		)
			
		$.each( barData, ( i, data ) ->	
			data.index = barData.length - 1 - i
			data.legendIndex = i
		)		

		spec = scope.spec
		title = "CommBiz Asset Finance"
		subtitle = "Story Deployment Cycle Time"
		storyList = jsonData.stories
		
		chart = new App.CycleChart()
		chart.render(spec, title, subtitle, storyList, barData)
		
	