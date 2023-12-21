import { createFetch } from "@vueuse/core";

const useMyFetch = createFetch({
  baseUrl: 'https://localhost:44356/api',
  combination: "overwrite",
  fetchOptions: {
    credentials: "include"
  }
})


export { useMyFetch };
