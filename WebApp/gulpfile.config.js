'use strict';

var GulpConfig = (function () {

    function gulpConfig() {

        this.source = './wwwroot/';
        this.sourceApp = this.source + '/scripts/';

        this.tsOutputPath = this.source + '/js';
        this.allJavaScript = [this.source + '/js/**/*.js'];
        this.appScript = this.sourceApp + 'app.ts';
        this.appConfigScript = this.sourceApp + 'app.config.ts';
        this.allTypeScript = this.sourceApp + '**/*.ts';
        this.allCompTypeScript = this.source +'comp/**/*.ts';

        this.typings = './typings/';
        //this.libraryTypeScriptDefinitions = './typings/**/*.ts';
        this.libraryTypeScriptDefinitions = './node_modules/@types/**/*.ts';
        this.customTypeScriptDefinitions = './typings/custom.d.ts';

        this.allSassFiles = [this.source + 'scss/**/*.scss'];
        this.cssOutputPath = this.source + '/css';
    }

    return gulpConfig;
})();

module.exports = GulpConfig;