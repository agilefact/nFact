$ -> 
	context = Singleton.get()
	controls = App.ControlsViewModel.create(context)

	vm = new App.Config(controls)
	context.bind(vm)
			
class App.Config
	constructor: (controls) ->
		@controls = controls

		@back = ->
			context = Singleton.get()
			context.navigateHome()

		@save = -> 
			context = Singleton.get()
			spec = context.getSelectedSpec()
			allInputs = $("div#divConfig :input").serializeArray()
			config = { Name: spec, appSettings: allInputs }
			encoded = JSON.stringify(config)
			url = "/" + spec + "/config"
			$.ajax(
				type: "POST",
				url: url,
				contentType: "application/json",
				dataType: "html",
				data: encoded,
				cache: false,
				success: (data) ->
					context.navigateHome()
				,
				error: (XMLHttpRequest, textStatus, errorThrown) -> 
					$("#spnMessage").html("An error occurred saving the configuration file.")
				)