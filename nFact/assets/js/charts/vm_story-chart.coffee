$ -> 
	json = $('#dataModel').html()
	dataModel = jQuery.parseJSON(json)
	
	vm = new App.Chart(dataModel)
	ko.applyBindings(vm)

	vm.create()
			
class App.Chart
	constructor: (model) ->
		@spec = model.spec
		@storyId = model.storyId
		@back = ->
			url = "/" + @spec + "/cycle"
			window.location.replace(url)
			
	create: ->
		urlChart = "/" + @spec + "/story/" + @storyId + "/chart?format=json"
		urlBar = "/" + @spec + "/story/" + @storyId + "/cycle?format=json"
		this.getData(urlChart, this.render, this)
		this.getData(urlBar, this.renderBar, this)
	
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
			
	createLabels: (environment, index) ->
		displayLabel = ->
			return environment

		if index == 0
			{enabled: true, formatter: displayLabel }
		else
			{}

	createMarker: (pt) ->
		if pt.accepted
			marker = {enabled: pt.enabled, symbol: 'square', radius: 5}
		else
			{enabled: pt.enabled, symbol: 'circle'}

	getDate: (date) ->
		year = date.getUTCFullYear()
		month = date.getUTCMonth()
		day = date.getUTCDate()
		Date.UTC(year, month, day)

	renderBar: (jsonData, scope) ->
		barData = []
		maxDays = 0
		$.each( jsonData.environmentCycle, ( index, environment ) ->	
			days = []
			color = scope.pickColor(index)
			$.each( environment.CycleDurations, ( index, duration ) ->	
				days.push({y: duration.days, color: color})
			)

			barData.push({name: environment.name, data: days, color: color})
			maxDays += days[0].y
		)
			
		$.each( barData, ( i, data ) ->	
			data.index = barData.length - 1 - i
			data.legendIndex = i
		)		

		chart = new App.CycleDuraton()

		spec = scope.spec
		title = "CommBiz - Team Top Gun"
		subTitle = "<b>Story Test Automation: </b>" + jsonData.storyName
		storyList = jsonData.stories

		chart.render(spec, title, subTitle, storyList, barData, maxDays)

	getTestRun: (environment, testRun, scope) ->
		result = {}
		$.each( scope.storyData, ( index, series ) ->
			environment = series.environment
			$.each( series.points, ( index, pt ) ->
				if pt.testRun == testRun && environment == environment
					result = pt
			)
		)
		return result
	
	pickColor: (i) ->
		colors = colors = ['#D1590F', '#0E7EC4', '#4FE86E', '#21B53E', '#036617']
		colors[i]
				
	render: (jsonData, scope) ->
		self = scope

		scope.storyData = jsonData.seriesArray
		chartData = []
		
		index = 0	
		barData = []
		minDate = null
		$.each( jsonData.seriesArray, ( index, series ) ->
			lineData = []
			barCategories = []
			
			environment = series.environment
			barCategories.push(environment)

			color = scope.pickColor(index)

			$.each( series.points, ( index, pt ) ->
				date = new Date(parseInt(pt.x.substr(6)))
				if minDate == null
					minDate = self.getDate(date)

				dataLabels = self.createLabels(environment, index)
				marker = self.createMarker(pt)
				
				utcDate = self.getDate(date)
				if pt.accepted
					barData.push({name: environment, data: [utcDate]})

				lineData.push({x: date, y: pt.y, marker: marker, dataLabels: dataLabels, id: pt.testRun, color: color})
			)
			chartData.push({name: environment, legendIndex: index, data: lineData, dataGrouping: {enabled: false}, color: color})
			index++;
		)
		
		$("#chart-line").highcharts
			title:
				text: ""
			xAxis:
				type: 'datetime'
				minTickInterval: 24 * 3600 * 1000
                
				labels: 
					formatter: ->
						Highcharts.dateFormat('%d %b', this.value)

			yAxis:
				title: 
					text: 'Environments'
				min: 0
				tickInterval: 1
				gridLineWidth: 0
				labels:
					enabled: false	

			legend:
				layout: "vertical"
				align: "right"
				verticalAlign: "middle"
				borderWidth: 0
				reversed: true

			plotOptions:
				line:
					marker:
						symbol: 'circle'
				series:
					cursor: 'pointer'
					point:
						events:
							click: (event) -> 
								testRun = this.id
								spec = self.spec
								url = "/" + spec + "/" + testRun
			tooltip:
				formatter: -> 
					data = scope.getTestRun(this.series.name, this.point.id, scope)
					if data != null
						data.result
					
			series: chartData

		