class App.CycleChart
	render: (spec, title, subTitle, stories, data, maxDays) ->
		console.log(spec)
		$("#chart-bar").highcharts
			chart:
				type: 'columnrange',
				inverted: true

			title:
				text: title
				x: -20 #center

			subtitle:
				text: subTitle
				x: -20

			yAxis:
				type: 'datetime'
				title: 
					text: 'Time'

			xAxis:
				categories: stories

			tooltip:
				valueSuffix: ' days'
													

			series: data

			legend:
				layout: "vertical"
				align: "right"
				verticalAlign: "middle"
				borderWidth: 0
			

		
	