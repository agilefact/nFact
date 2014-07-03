$ -> 
	vm = new App.ProjectChart()
	vm.create()
			
class App.ProjectChart
	create: ->
		json = $('#dataModel').html()
		dataModel = jQuery.parseJSON(json)
		@spec = dataModel.spec
		urlData = "/" + @spec + "/deployment?format=json"
		this.getData(urlData, this.render, this)
	
	
	pickColor: (i) ->
		colors = ['#D1590F', '#0E7EC4', '#4FE86E', '#21B53E', '#036617']
		colors[i]

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
			color = scope.pickColor(index)
			$.each( environment.CycleDurations, ( index, duration ) ->	
				days.push({y: duration.days, color: color})
			)

			barData.push({name: environment.name, data: days, color: color})
		)
			
		$.each( barData, ( i, data ) ->	
			data.index = barData.length - 1 - i
			data.legendIndex = i
		)		

		spec = scope.spec
		title = "CommBiz Asset Finance"
		subtitle = "Deployment Cycle Time"
		storyList = jsonData.stories
		
		chart = new App.CycleDuraton()
		chart.render(spec, title, subtitle, storyList, barData)
		
	