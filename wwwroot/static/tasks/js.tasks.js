module.exports = function (gulp, $) {

  //
  // JS Linter
  //
  gulp.task('js-lint', function () {
    return gulp.src('static/scripts/all.js')
    .pipe($.eslint({
      configFile: '.eslintrc'
    }))
    .pipe($.eslint.format())
    .pipe($.eslint.failAfterError());
  });
}