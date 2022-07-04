module.exports = function (gulp, $) {

  //
  // Create image sprite
  //
  gulp.task('sprite', function() {

    var spritePadding = 0;

    return gulp.src(['**/sprite-*.svg'], {
          cwd : 'static/images/icons/'
        })
        .pipe($.svgSprite({
          shape : {
            id : {},
            dimension : {
              precision   : 2,
              attributes  : false,
            },
            spacing : {
              padding     : spritePadding,
            },
          },
          mode : {
            /*
            * Standard svg sprite with matching CSS map
            */
            css : {
              dest: 'static/css/',
              sprite : '../images/sprite/sprite.svg',
              prefix: '.',
              bust : false,
              render: {
                scss: {
                  dest: '../../static/css/maps/_svg-sprite.scss',
                  template: 'static/css/maps/_svg-sprite-template.scss',
                }
              }
            },
            /*
            * SVG Symbol defintions to use <use xhref="#"> etc )
            */
            symbol: { // symbol mode to dist the SVG
              render: {
                css: false,
                scss: false
              },
              dest: 'Views/Shared/',
              prefix: '.svg--%s',
              sprite: 'SVG_Sprite.cshtml',
              example: false
            }
          },
          svg: {
            namespaceClassnames : false
          },
          variables : {
            // Custom variables we use in our svg-sprite-template.scss
            png : function() {
              return function(sprite, render) {
                return render(sprite).split('.svg').join('.png');
              }
            },
            // See this issue https://github.com/jkphl/svg-sprite/issues/134
            // To compensate for your padding value i had to adjust the background-position manually.
            correctPosition: function() {
              return function(backgroundPosition, render) {
                var backgroundPosition = render(backgroundPosition);
                return parseFloat(backgroundPosition - spritePadding)+ "px";
              }
            }
          }
        }))
        .pipe(gulp.dest('.'))
        .pipe($.filter("static/images/sprite/*.svg"))
        .pipe($.raster())
        .pipe($.rename({extname: '.png'}))
        .pipe(gulp.dest('.'));
  });
}