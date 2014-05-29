$ -> 
	context = Singleton.get()
	controls = App.ControlsViewModel.create(context)

	vm = new App.Index(controls)
	context.bind(vm)
			
class App.Index
	constructor: (controls) ->
		@controls = controls	
		@toggleControls = ->
			if controls.isVisible()
				controls.hide()
			else
				controls.show()
			