'use strict';

var gulp = require('gulp'),
    debug = require('gulp-debug'),
    Config = require('./gulpfile.config'),
    sass = require('gulp-sass');

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
