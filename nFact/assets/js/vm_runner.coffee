$ -> 
	context = Singleton.get()
	controls = App.ControlsViewModel.create(context)

	vm = new App.Runner(controls)
	context.bind(vm)

	vm.getOutput()

class App.Runner
	constructor: (controls) ->
		@controls = controls
		@stop = -> 
			context = Singleton.get()
			url = "/" + context.getSelectedSpec() + "/stop"
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

	CURRENT_STATE: 'OK'

	run: -> 
		context = Singleton.get()
		url = "/" + context.getSelectedSpec() + "/run"
		environment = getSelectedEnvironment()
		if (environment != null)
			url = url + "?environment=" + environment

		$.ajax(
			type: "POST",
			url: url,
			contentType: "application/json"
			dataType: "html",
			data: "",
			cache: false,
			success: (data) ->
				getOutput()
			,
			error: (XMLHttpRequest, textStatus, errorThrown) -> 
			)

	getOutput: -> 
		context = Singleton.get()
		url = "/" + context.getSelectedSpec() + "/output"
		_self = this
		$.ajax(
			type: "Get",
			url: url,
			contentType: "application/json"
			dataType: "html",
			data: "",
			async: true,
			cache: false,
			success: (data) ->
				msg = jQuery.parseJSON(data);
				CURRENT_STATE = msg.State
				switch (CURRENT_STATE)
					when 'OK', 'Stopping'
						addMessage('new', msg.Message)
						setTimeout(
							() -> _self.getOutput(), # Request next message
							1000       # after 1 seconds 
						)
					when 'Error' then addMessage('error', msg.Message)
					when 'Completed', 'Stopped' 
						addMessage('new', msg.Message)
						setTimeout(
							() -> context.navigateHome(), # return to main page
							2000        
						)
					else 
						addMessage('new', msg.Message)
			,
			error: (XMLHttpRequest, textStatus, errorThrown) -> 
				addMessage('Lost connection to server..', textStatus)
				addMessage('Lost connection to server..', errorThrown)
			)
	
	addMessage = (type, message) ->
		if (message == '')
			return
		
		$('#divMessages').append("<div class='msg " + type + "'>" + message + "</div>")
		
		x = 0
		doc = document.documentElement
		y = doc.clientHeight
		window.scroll(x, y)
		