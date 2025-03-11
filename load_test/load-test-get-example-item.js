import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
    stages: [
        { duration: '30s', target: 10 }, // Ramp-up to 10 users over 30 seconds
        { duration: '1m', target: 50 }, // Stay at 50 users for 1 minute
        { duration: '30s', target: 0 }, // Ramp-down to 0 users
    ],

    insecureSkipTLSVerify: true, // Skip TLS verification for self-signed certificates
};

export default function () {
    const url = 'https://localhost:7298/item/99';
    
    const res = http.get(url);

    // Check if the response status is 200
    check(res, {
        'is status 200': (r) => r.status === 200,
    });

    sleep(1); // Wait for 1 second between requests
}
