//
// Load tasks
//
var gulp = require('gulp');

//
// Load all the plugins into the $ variable
//
var $ = require('gulp-load-plugins')({
  pattern: '*'
});

//
// Pass the plugins along so that your tasks can use them
// Will load all xxx.tasks.js files
//
$.loadSubtasks('static/tasks/', $);

//
// Watch
//
gulp.task('watch', function () {
  gulp.watch(['./static/css/**/*.scss'], { verbose: true }, ['libsass', 'rte-libsass', 'firstpaint', 'sass-lint']);
  gulp.watch(['./static/scripts/*.js'], { verbose: true }, ['js-lint']);
  gulp.watch(['./static/images/icons/*.svg'], { verbose: true }, ['sprite']);
});

//
// Help
//
gulp.task('help', function() {
console.log('HELP: The following functions can be used currently: '+'\n\n');
console.log('Command: "gulp default" - Our normal build. Will build JS stuff, then go onto watching less as usual. Builds with ruby sass.'+'\n');
console.log('Command: "gulp build" - Build website for production. This includes additional minification of assets.'+'\n');
});

//
// Tasks
//
gulp.task('default', ['firstpaint', 'sprite', 'libsass', 'watch']);
gulp.task('build', ['firstpaint', 'sprite', 'libsass', 'watch']);