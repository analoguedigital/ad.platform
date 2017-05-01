'use strict';

var gulp = require('gulp'),
    debug = require('gulp-debug'),
    inject = require('gulp-inject'),
    tsc = require('gulp-typescript'),
    tslint = require('gulp-tslint'),
    sourcemaps = require('gulp-sourcemaps'),
    rimraf = require('gulp-rimraf'),
    Config = require('./gulpfile.config'),
    concat = require('gulp-concat'),
    tsProject = tsc.createProject('tsconfig.json'),
    sass = require('gulp-sass');

var config = new Config();

/**
 * Lint all custom TypeScript files.
 */
gulp.task('ts-lint', function () {
    return gulp.src(config.allTypeScript).pipe(tslint()).pipe(tslint.report({ formatter: 'msbuild' }));
});

/**
 * Compile TypeScript and include references to library and app .d.ts files.
 */
gulp.task('compile-ts', function () {
    var sourceTsFiles = [config.libraryTypeScriptDefinitions,//reference to library .d.ts files
    config.customTypeScriptDefinitions,
    config.appScript,
    config.appConfigScript,
    config.allTypeScript,                //path to typescript files
    config.allCompTypeScript];


    var tsResult = gulp.src(sourceTsFiles)
        .pipe(sourcemaps.init())
        .pipe(tsProject());

    tsResult.dts.pipe(gulp.dest(config.tsOutputPath));
    return tsResult.js
        .pipe(concat('scripts.js'))
        .pipe(sourcemaps.write('.', { includeContent: true, sourceRoot: '/wwwroot/' }))
        .pipe(gulp.dest(config.tsOutputPath));
});

/**
 * Remove all generated JavaScript files from TypeScript compilation.
 */
gulp.task('clean-ts', function () {
    var typeScriptGenFiles = [config.tsOutputPath,            // path to generated JS files
    config.sourceApp + '**/*.js',    // path to all JS files auto gen'd by editor
    config.sourceApp + '**/*.js.map' // path to all sourcemap files auto gen'd by editor
    ];

    // delete the files
    return gulp.src(typeScriptGenFiles, { read: false })
        .pipe(rimraf());
});


gulp.task('sass', function () {
    gulp.src(config.allSassFiles)
        .pipe(sass({ outputStyle: 'compressed' }))
        .pipe(gulp.dest(config.cssOutputPath));
});


gulp.task('watch', function () {
    //var sourceTsFiles = [config.appScript,
    //config.appConfigScript,
    //config.allTypeScript,                //path to typescript files
    //config.allCompTypeScript,
    //config.libraryTypeScriptDefinitions]; //reference to library .d.ts files
    //gulp.watch(sourceTsFiles, ['ts-lint', 'compile-ts']);
    gulp.watch(config.allSassFiles, ['sass']);
});


gulp.task('default', [
    //'ts-lint',
    //'compile-ts',
    'sass',
    'watch']);
