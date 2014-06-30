class App.CycleChart
	render: (spec, title, subTitle, stories, data) ->
		console.log(spec)
		$("#chart-bar").highcharts
			chart:
				type: 'bar'

			title:
				text: title
				x: -20 #center

			subtitle:
				text: subTitle
				x: -20

			yAxis:
				title: 
					text: 'Days'
				endOnTick: false
				opposite: true

			xAxis:
				categories: stories
			
			plotOptions:
				bar:
					stacking: 'normal'
					pointWidth: 20	
				series:
					cursor: 'pointer'
					point:
						events:
							click: (event) -> 
								storyId = this.category
								url = "/" + spec + "/story/" + storyId + "/chart"
								window.location.replace(url)
								

			series: data

			legend:
				layout: "vertical"
				align: "right"
				verticalAlign: "middle"
				borderWidth: 0
			

		
	