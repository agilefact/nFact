﻿$ -> 
	vm = new App.Chart()
	vm.create()
			
class App.Chart
	create: ->
		json = $('#dataModel').html()
		dataModel = jQuery.parseJSON(json)
		spec = dataModel.spec
		storyId = "US39"
		url = "/" + spec + "/story/" + storyId + "/chart?format=json"
		this.getData(url, this.render)
	
	getData: (url, callback) ->
		$.ajax
			type: "GET",
			url: url,
			contentType: "application/json"
			dataType: "html",
			data: "",
			cache: false,
			success: (result) ->
				data = JSON.parse(result)
				callback(data)
			,
			error: (XMLHttpRequest, textStatus, errorThrown) -> 
				alert('error occurred')
			

	render: (jsonData) ->
		chartData = []
		
		index = 0	
		$.each( jsonData.seriesArray, ( index, series ) ->
			data = []
			$.each( series.points, ( index, pt ) ->
				date = new Date(parseInt(pt.x.substr(6)))

				marker = {enabled: pt.enabled, symbol: 'circle'}
				if pt.accepted
					marker = {enabled: pt.enabled, symbol: 'square', radius: 5}

				data.push({x: date, y: pt.y, marker: marker})
			)
			environment = series.environment
			chartData.push({name: environment, legendIndex: index, data})
			index++;
		)

		$("#chart-container").highcharts
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
				min: 0
				tickInterval: 1

				plotLines: [
					value: 0
					width: 1
					color: "#808080"
				]		

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

		