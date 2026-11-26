// Browser check for the image links: one per line, each must be an http or https link.
// The server checks the same rule again with Regex.IsMatch.
document.addEventListener('DOMContentLoaded', function () {
  var form = document.querySelector('form[data-image-check]');
  if (!form) return;

  var box = form.querySelector('textarea[name="ImageUrls"]');
  var message = form.querySelector('span[data-valmsg-for="ImageUrls"]');
  var pattern = /^https?:\/\/\S+$/;

  // Returns an error message, or "" when every line is fine.
  function check() {
    var lines = box.value.split('\n')
      .map(function (line) { return line.trim(); })
      .filter(function (line) { return line !== ''; });

    for (var i = 0; i < lines.length; i++) {
      if (!pattern.test(lines[i])) {
        return 'Line ' + (i + 1) + ' is not a valid image link. It must start with http:// or https://.';
      }
    }
    return '';
  }

  form.addEventListener('submit', function (event) {
    var error = check();
    message.textContent = error;
    if (error !== '') {
      event.preventDefault();   // do not send the form
      box.focus();
    }
  });

  box.addEventListener('input', function () { message.textContent = ''; });
});