// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.

export const environment = {
  production: false,
  pickemRestServerBaseUrl: "http://localhost:51890/api",
  pickemBaseDomain: "localhost:51890",
  pickemWebSocketUrl: "ws://localhost:51890/ws",
};
