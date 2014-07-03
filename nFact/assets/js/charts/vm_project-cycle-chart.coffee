$ -> 
	vm = new App.ProjectChart()
	vm.create()
			
class App.ProjectChart
	create: ->
		json = $('#dataModel').html()
		dataModel = jQuery.parseJSON(json)
		@spec = dataModel.spec
		urlData = "/" + @spec + "/cycle?format=json"
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

	getDate: (strDate) ->
		date = new Date(parseInt(strDate.substr(6)))
		year = date.getUTCFullYear()
		month = date.getUTCMonth()
		day = date.getUTCDate()
		Date.UTC(year, month, day)

	render: (jsonData, scope) ->
		barData = []
		maxDays = 0
		$.each( jsonData.environmentCycle, ( index, environment ) ->	
			days = []
			color = scope.pickColor(index)
			$.each( environment.CycleDurations, ( index, duration ) ->	
				start = scope.getDate(duration.start)
				end = scope.getDate(duration.end)
				days.push({low: start, high: end, color: color})
			)

			barData.push({name: environment.name, data: days, color: color})
		)	

		spec = scope.spec
		title = "CommBiz Asset Finance"
		subtitle = "Story Cycle Time"
		storyList = jsonData.stories
		
		chart = new App.CycleChart()
		chart.render(spec, title, subtitle, ['s1'], barData)
		
	