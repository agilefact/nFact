window.App = {}
root = exports ? this

class Context
	constructor: (@dataModel) ->
		
	bind: (dataModel) ->
		@dataModel = dataModel
		@controls = @dataModel.controls
		
		# bind to page
		ko.applyBindings(dataModel)

	getSelectedSpec: () -> 
		@controls.selectedSpec()
	getSelectedEnvironment: () ->
		@controls.selectedEnvironment();
	navigateHome: () ->
		this.navigate("/")
	navigateToSpec: (spec) ->
		this.navigate("/" + spec)
	navigateToNode: (node) ->
		spec = @controls.selectedSpec()
		this.navigate("/" + spec + "/" + node)
	navigate: (url) ->
		$.when(this.save()).then(() ->
			window.location.replace(url)
		)
		
	save: () ->
		data = ko.toJSON(this.controls)
		send("/settings/", data)
	send = (url, json) ->
		sentSuccess = $.Deferred()
		$.ajax(
			type: "POST",
			url: url,
			contentType: "application/json",
			dataType: "html",
			data: json,
			cache: false,
			success: () ->
				sentSuccess.resolve()
			error: (XMLHttpRequest, textStatus, errorThrown) -> 
				$("#spnMessage").html("An error occurred.")
			)
		sentSuccess.promise()
	

class Singleton
	instance = null
	
	# This is a static method used to either retrieve the
	# instance or create a new one.
	
	@get: () ->
		instance ?= new Context()

# Export Singleton as a module
root.Singleton = Singleton