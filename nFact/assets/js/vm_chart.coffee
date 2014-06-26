$ -> 
	vm = new App.Chart()
	vm.create()
			
class App.Chart
	create: ->
		json = $('#dataModel').html()
		dataModel = jQuery.parseJSON(json)
		spec = dataModel.spec
		storyId = "12345"
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
			

	render: (data) ->
		series = data.series
		data1 = []
		$.each( series.points, ( index, pt ) ->
			date = new Date(parseInt(pt.x.substr(6)))
			data1.push({x: date, y: pt.y})
		)

		$("#chart-container").highcharts
			title:
				text: "Monthly Average Temperature"
				x: -20 #center

			subtitle:
				text: "Source: WorldClimate.com"
				x: -20

			xAxis:
				type: 'datetime'
				tickInterval: 24 * 3600 * 1000
                
				labels: 
					formatter: ->
						Highcharts.dateFormat('%a %d %b', this.value)

			yAxis:
				title: "My Title"
				text: "Temperature (°C)"

				plotLines: [
					value: 0
					width: 1
					color: "#808080"
				]

			tooltip:
				valueSuffix: "°C"

			legend:
				layout: "vertical"
				align: "right"
				verticalAlign: "middle"
				borderWidth: 0

			series: [
				name: "local"
				data: data1
			]

		