class App.ControlsViewModel
	constructor: () ->
		@video = ko.observable()
		@steps = ko.observable()
		@selectedSpec = ko.observable()
		@specs = []
		@environments = []
		@selectedEnvironment = ko.observable()
		@navigate = ko.observable()
		@isVisible = ko.observable(false)
		@run = ->
			this.navigateEnvironment('run')
		@restart = ->
			context = Singleton.get()
			url = "/" + this.selectedSpec() + "/restart"
			$.ajax(
				type: "POST",
				url: url,
				contentType: "application/json"
				dataType: "html",
				data: "",
				cache: false,
				error: (XMLHttpRequest, textStatus, errorThrown) -> 
					setTimeout(
							() -> context.navigateHome(), # return to main page
							5000        
						)
				)
		@navigateEnvironment = (node) ->
			context = Singleton.get()
			url = "/" + this.selectedSpec() + "/" + node
			environment = this.selectedEnvironment()
			if environment != null
				url = url + "?environment=" + environment
			context.navigate(url)
		@isVisible = ko.observable(false)
		@hide = ->
			context = Singleton.get()
			context.save()
			this.isVisible(false)
			$("#divControls").slideUp("fast")
		@show = ->
			this.isVisible(true)
			$("#divControls").slideDown("fast")

	@create: (context) ->
		# capture current state 
		state = ko.mapping.toJS(new App.ControlsViewModel());
	
		json = $('#dataModel').html()
		dataModel = jQuery.parseJSON(json)
	
		state.specs = dataModel.specs
		state.selectedSpec = dataModel.selectedSpec
		state.video = dataModel.video
		state.steps = dataModel.steps
		state.environments = dataModel.environments
		state.selectedEnvironment = dataModel.selectedEnvironment

		# create view model initialised with current state
		controls = ko.mapping.fromJS(state);
		
		# On selected spec changed event navigate to new page 
		controls.selectedSpec.subscribe( (spec) ->
			context.navigateToSpec(spec)
		)

		# navigate event
		controls.navigate.subscribe( (node) ->
			context.navigateToNode(node)
		)

		return controls


