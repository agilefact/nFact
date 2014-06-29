$ -> 
	vm = new App.Chart()
	vm.create()
			
class App.Chart
	create: ->
		json = $('#dataModel').html()
		dataModel = jQuery.parseJSON(json)
		spec = dataModel.spec
		storyId = "US39"
		urlChart = "/" + spec + "/story/" + storyId + "/chart?format=json"
		urlBar = "/" + spec + "/story/" + storyId + "/cycle?format=json"
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
		year = date.getFullYear()
		month = date.getMonth()
		day = date.getDate()
		Date.UTC(year, month, day)

	renderBar: (jsonData, scope) ->
		self = scope
		barData = []
		maxDays = 0
		$.each( jsonData.environmentCycleTime, ( index, cycle ) ->	
			barData.push({name: cycle.name, data: [cycle.days]})
			maxDays += cycle.days
		)
			
		$.each( barData, ( i, data ) ->	
			data.index = barData.length - 1 - i
			data.legendIndex = i
		)		

		$("#chart-bar").highcharts
			chart:
				type: 'bar'

			title:
				text: "Story Test Automation"
				x: -20 #center

			subtitle:
				text: jsonData.storyName
				x: -20

			yAxis:
				title: 
					text: 'Days'
				max: maxDays
				endOnTick: false
				opposite: true

			xAxis:
				categories: ['US39']
			
			plotOptions:
				bar:
					stacking: 'normal'
					pointWidth: 20				

			series: barData

			legend:
				layout: "vertical"
				align: "right"
				verticalAlign: "middle"
				borderWidth: 0
			

		
	render: (jsonData, scope) ->
		self = scope
		chartData = []
		
		index = 0	
		barData = []
		minDate = null
		$.each( jsonData.seriesArray, ( index, series ) ->
			lineData = []
			barCategories = []
			
			environment = series.environment
			barCategories.push(environment)

			$.each( series.points, ( index, pt ) ->
				date = new Date(parseInt(pt.x.substr(6)))
				if minDate == null
					minDate = self.getDate(date)

				dataLabels = self.createLabels(environment, index)
				marker = self.createMarker(pt)
				
				utcDate = self.getDate(date)
				if pt.accepted
					barData.push({name: environment, data: [utcDate]})

				lineData.push({x: date, y: pt.y, marker: marker, dataLabels: dataLabels})
			)
			chartData.push({name: environment, legendIndex: index, data: lineData})
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
					
					
			series: chartData

		