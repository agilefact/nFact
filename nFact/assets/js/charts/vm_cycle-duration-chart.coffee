class App.CycleDuraton
	render: (spec, title, subTitle, stories, data, maxDays) ->
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
				max: maxDays
				title: 
					text: 'Days'
				endOnTick: false
				opposite: true
				tickInterval: 1

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
			tooltip:
				valueSuffix: ' days'
													

			series: data

			legend:
				layout: "vertical"
				align: "right"
				verticalAlign: "middle"
				borderWidth: 0
			

		
	