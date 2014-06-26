$ -> 
	context = Singleton.get()
	controls = App.ControlsViewModel.create(context)

	vm = new App.Index(context, controls)
	context.bind(vm)
			
class App.Index
	constructor: (context, controls) ->
		@context = context
		@controls = controls	
		@toggleControls = ->
			if controls.isVisible()
				controls.hide()
			else
				controls.show()

		@accept = (storyId) ->
			spec = context.getSelectedSpec()
			test = context.getTestRun()
			url = "/" + spec + "/stories/" + storyId + "/test/" + test + "/accept"

			$.ajax(
				type: "POST",
				url: url,
				contentType: "application/json"
				dataType: "html",
				data: "",
				cache: false,
				success: (data) ->
					btn = "button[story-id='" + storyId + "']"
					$(btn).html('Accepted');
					$(btn).attr('Disabled', 'True');
				,
				error: (XMLHttpRequest, textStatus, errorThrown) -> 
				)
			