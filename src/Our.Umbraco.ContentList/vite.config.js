export default {
  build: {
    outDir: "wwwroot/scripts",
    minify: false,
    watch: {},
    lib: {
      name: "Our Umbraco ContentList",
      entry: "Scripts/main.js"
    }
  }
}