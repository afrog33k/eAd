/******************************************
 * jquery.dim.js
 * natebeacham 2010
 *****************************************/

$.fn.dim = function(options) {
	var that = this;
	
	var defaults = {
		step: 20,
		property: 'background-color'
	};
	
	$.extend(this, defaults, options);
	
	var colors = this.css(this.property)
		.replace('rgb', '')
		.replace('(', '')
		.replace(')', '')
		.split(',');
	
	$.each(colors, function(i, color) {
		colors[i] = parseInt(color, 10) - that.step;
		if (colors[i] < 0) {
			colors[i] = 0;
		}
	})
	
	this.css(
		this.property,
		'rgb(' + colors.join(', ') + ')'
	)
}

$.fn.brighten = function(options) {
	var that = this;
	
	var defaults = {
		step: 20,
		property: 'background-color'
	};
	
	$.extend(this, defaults, options);
	
	var colors = this.css(this.property)
		.replace('rgb', '')
		.replace('(', '')
		.replace(')', '')
		.split(',');
	
	$.each(colors, function(i, color) {
		colors[i] = parseInt(color, 10) + that.step;
		if (colors[i] > 255) {
			colors[i] = 255;
		}
	})
	
	this.css(
		this.property,
		'rgb(' + colors.join(', ') + ')'
	)
}