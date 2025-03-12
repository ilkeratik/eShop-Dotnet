## I have generated a load test for GET /item/{id} running on Catalog API
It runs without authentication and tries getting the item page
Run using: `k6 run load-test.js`
The output for this test saved in the file: `output-load-test-get-example-item.txt`

##### I also tried to generate a load test for basket API but the API requires authentication, the application handles authentication via cookies, and I tried to copy and replicate the request but it didn't work, resulted in all requests failing
- I added an example of how that request made on browser in the file `cmd-basket-api.js`