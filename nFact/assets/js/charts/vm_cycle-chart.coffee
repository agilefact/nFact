class App.CycleChart
	render: (spec, title, subTitle, stories, data, labelText) ->
		console.log(spec)
		$("#container").highcharts
			chart:
				type: 'columnrange'
				inverted: true
				marginRight: 250
				events:
					load: ->
						label = this.renderer.label(labelText)
						label.css({width: '260px'})
						attr =
							stroke: 'silver'
							'stroke-width': 1
							r: 2
							padding: 10

						label.attr(attr)
						label.add();

						align =
							align: 'right'
							x: -20
							y: 100
							verticalAlign: 'top'

						extend = Highcharts.extend(label.getBBox(), align)
						label.align(extend, null, 'spacingBox')

			title:
				text: title
				x: -20 #center

			subtitle:
				text: subTitle
				x: -20

			yAxis:
				max: Date.UTC(2014, 07, 05)
				type: 'datetime'
				title: 
					text: 'Time'
				
				plotLines: [
					color: 'red'
					value: Date.UTC(2014, 07, 01)
					dashStyle : 'longdash'
					width : 1
					label:
						color: 'red'
						text: 'Release 35'
						verticalAlign: 'bottom'
						textAlign: 'right'
						y: -50
					]

			xAxis:
				categories: stories

			tooltip:
				formatter: -> 
					'<b>' + this.key + 
						': </b>' + 
						Highcharts.dateFormat('%d %b', this.point.low) + ' - ' +
						Highcharts.dateFormat('%d %b', this.point.high) 
													

			series: data

			plotOptions:
				bar:
					pointWidth: 20	
				series:
					cursor: 'pointer'
					point:
						events:
							click: (event) -> 
								storyId = this.category
								url = "/" + spec + "/story/" + storyId + "/chart"
								
								if this.color == "#0E7EC4"
									window.location.href = url

			legend:
				layout: "vertical"
				align: "right"
				verticalAlign: "top"
				y: 50
				x: -80
				borderWidth: 0
			

		
	