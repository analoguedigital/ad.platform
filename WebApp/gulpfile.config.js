'use strict';

var GulpConfig = (function () {

    function gulpConfig() {

        this.source = './wwwroot/';
        this.allSassFiles = [this.source + 'scss/**/*.scss'];
        this.cssOutputPath = this.source + '/css';
    }

    return gulpConfig;
})();

module.exports = GulpConfig;