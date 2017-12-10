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
        .pipe(gulp.dest('wwwroot/dist/'));
});

gulp.task('directives', function () {
    return gulp.src('wwwroot/scripts/directives/**/*.html')
        .pipe(templateCache({
            root: 'scripts/directives',
            module: 'app',
            filename: 'app.directives-tpl.min.js'
        }))
        .pipe(gulp.dest('wwwroot/dist/'));
});

gulp.task('css', function () {
    return domSrc({ file: 'wwwroot/index.html', selector: 'link', attribute: 'href' })
        .pipe(concat('app.full.min.css'))
        .pipe(cssmin())
        .pipe(gulp.dest('wwwroot/dist/'));
});

gulp.task('js', function () {
    return domSrc({ file: 'wwwroot/index.html', selector: 'script', attribute: 'src' })
        .pipe(concat('app.full.min.js'))
        .pipe(uglify({
            mangle: false,
            compress: false
        }))
        .pipe(gulp.dest('wwwroot/dist/'));
});

gulp.task('indexHtml', function () {
    return gulp.src('wwwroot/index.html')
        .pipe(cheerio(function ($) {
            $('script').remove();
            $('link').remove();
            $('body').append('<script src="app.full.min.js"></script>');
            $('body').append('<script src="app.components-tpl.min.js"></script>');
            $('body').append('<script src="app.directives-tpl.min.js"></script>');
            $('body').append("<script src='//maps.googleapis.com/maps/api/js?key=AIzaSyCESw4TSMh0XnrLzrdpoBgxNX_iZiVKqSk'></script>");
            $('head').append('<link rel="stylesheet" href="app.full.min.css">');
        }))
        .pipe(gulp.dest('wwwroot/dist/'));
});

gulp.task('build', ['css', 'templates', 'directives', 'js', 'indexHtml']);