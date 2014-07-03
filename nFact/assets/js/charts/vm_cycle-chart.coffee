class App.CycleChart
	render: (spec, title, subTitle, stories, data, annotations) ->
		console.log(spec)
		$("#container").highcharts
			annotationsOptions:
				enabledButtons: false
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
				formatter: -> 
					'<b>' + this.key + 
						': </b>' + 
						Highcharts.dateFormat('%d %b', this.point.low) + ' - ' +
						Highcharts.dateFormat('%d %b', this.point.high) 
													

			series: data

			legend:
				layout: "vertical"
				align: "right"
				verticalAlign: "middle"
				borderWidth: 0
			annotations: annotations
			

		
	