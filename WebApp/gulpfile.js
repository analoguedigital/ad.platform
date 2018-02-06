'use strict';

var gulp = require('gulp');
var domSrc = require('gulp-dom-src');
var concat = require('gulp-concat');
var cssmin = require('gulp-cssmin');
var uglify = require('gulp-uglify');
var cheerio = require('gulp-cheerio');
var templateCache = require('gulp-angular-templatecache');

var debug = require('gulp-debug');
var Config = require('./gulpfile.config');
var sass = require('gulp-sass');

var config = new Config();

/**
 * Lint all custom TypeScript files.
 */
gulp.task('ts-lint', function () {
    return gulp.src(config.allTypeScript).pipe(tslint()).pipe(tslint.report({ formatter: 'msbuild' }));
});


gulp.task('sass', function () {
    gulp.src(config.allSassFiles)
        .pipe(sass({ outputStyle: 'compressed' }))
        .pipe(gulp.dest(config.cssOutputPath));
});


gulp.task('watch', function () {
    gulp.watch(config.allSassFiles, ['sass']);
});

gulp.task('default', [
    'sass',
    'watch']);


gulp.task('templates', function () {
    return gulp.src('wwwroot/comp/**/*.html')
        .pipe(templateCache({
            root: 'comp',
            module: 'app',
            filename: 'app.components-tpl.min.js'
        }))
        .pipe(gulp.dest('wwwroot/dist/js/'));
});

gulp.task('directives', function () {
    return gulp.src('wwwroot/scripts/directives/**/*.html')
        .pipe(templateCache({
            root: 'scripts/directives',
            module: 'app',
            filename: 'app.directives-tpl.min.js'
        }))
        .pipe(gulp.dest('wwwroot/dist/js/'));
});

gulp.task('css', function () {
    return domSrc({ file: 'wwwroot/index.html', selector: 'link', attribute: 'href' })
        .pipe(concat('app.full.min.css'))
        .pipe(cssmin())
        .pipe(gulp.dest('wwwroot/dist/css/'));
});

gulp.task('js', function () {
    return domSrc({ file: 'wwwroot/index.html', selector: 'script', attribute: 'src' })
        .pipe(concat('app.full.min.js'))
        .pipe(uglify({
            mangle: false,
            compress: false
        }))
        .pipe(gulp.dest('wwwroot/dist/js/'));
});

gulp.task('copy', function () {
    gulp.src('wwwroot/favicon.png')
        .pipe(gulp.dest('wwwroot/dist/'));

    gulp.src('wwwroot/fonts/*.*')
        .pipe(gulp.dest('wwwroot/dist/fonts/'));

    gulp.src('wwwroot/img/**/*')
        .pipe(gulp.dest('wwwroot/dist/img/'));

    gulp.src('wwwroot/lib/**/*')
        .pipe(gulp.dest('wwwroot/dist/lib/'));

    gulp.src('wwwroot/uib/**/*')
        .pipe(gulp.dest('wwwroot/dist/uib/'));
});

gulp.task('indexHtml', function () {
    return gulp.src('wwwroot/index.html')
        .pipe(cheerio(function ($) {
            $('script').remove();
            $('link').remove();

            $('head').append('<link rel="apple-touch-icon" sizes="57x57" href="favicon.png">');
            $('head').append('<link rel="apple-touch-icon" sizes="60x60" href="favicon.png">');
            $('head').append('<link rel="apple-touch-icon" sizes="72x72" href="favicon.png">');
            $('head').append('<link rel="apple-touch-icon" sizes="76x76" href="favicon.png">');
            $('head').append('<link rel="apple-touch-icon" sizes="114x114" href="favicon.png">');
            $('head').append('<link rel="apple-touch-icon" sizes="120x120" href="favicon.png">');
            $('head').append('<link rel="apple-touch-icon" sizes="144x144" href="favicon.png">');
            $('head').append('<link rel="apple-touch-icon" sizes="152x152" href="favicon.png">');
            $('head').append('<link rel="apple-touch-icon" sizes="180x180" href="favicon.png">');
            $('head').append('<link rel="icon" type="image/png" sizes="192x192" href="favicon.png">');
            $('head').append('<link rel="icon" type="image/png" sizes="32x32" href="favicon.png">');
            $('head').append('<link rel="icon" type="image/png" sizes="96x96" href="favicon.png">');
            $('head').append('<link rel="icon" type="image/png" sizes="16x16" href="favicon.png">');

            $('head').append('<link rel="stylesheet" href="css/app.full.min.css"/>');
            $('head').append('<link href="https://code.ionicframework.com/ionicons/2.0.1/css/ionicons.min.css" rel="stylesheet"/>');

            $('body').append('<script src="js/app.full.min.js"></script>');
            $('body').append('<script src="js/app.components-tpl.min.js"></script>');
            $('body').append('<script src="js/app.directives-tpl.min.js"></script>');

            $('body').append('<script src="https://cdnjs.cloudflare.com/ajax/libs/animejs/2.0.2/anime.min.js"></script>');
            $('body').append('<script src="https://cdn.jsdelivr.net/particles.js/2.0.0/particles.min.js"></script>');
            $('body').append("<script src='//maps.googleapis.com/maps/api/js?key=AIzaSyCESw4TSMh0XnrLzrdpoBgxNX_iZiVKqSk'></script>");
            $('body').append('<script type="text/javascript" src="https://cdn.ywxi.net/js/1.js" async></script>');
        }))
        .pipe(gulp.dest('wwwroot/dist/'));
});

gulp.task('build', ['css', 'templates', 'directives', 'js', 'indexHtml', 'copy']);