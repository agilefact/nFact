$ -> 
	vm = new App.Chart()
	vm.create()
			
class App.Chart
	create: ->
		json = $('#dataModel').html()
		dataModel = jQuery.parseJSON(json)
		spec = dataModel.spec
		storyId = "US39"
		url = "/" + spec + "/story/" + storyId + "/chart?format=json"
		this.getData(url, this.render, this)
	
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



		$("#chart-bar").highcharts
			chart:
				type: 'column',
				inverted: true

			title:
				text: "Story Test Automation"
				x: -20 #center

			subtitle:
				text: jsonData.storyName
				x: -20

			xAxis:
				categories: ['US39']

			yAxis:
				#type: 'datetime'
				min:  minDate
						

			series: barData
			



		$("#chart-line").highcharts
			title:
				text: "Story Test Automation"
				x: -20 #center

			subtitle:
				text: jsonData.storyName
				x: -20

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

		