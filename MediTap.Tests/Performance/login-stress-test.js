import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {

    insecureSkipTLSVerify: true,

    stages: [
        { duration: '10s', target: 20 }, // Stage 1: Ramp up to 20 Virtual Users over 10 seconds
        { duration: '30s', target: 20 }, // Stage 2: Hold at 20 Virtual Users for 30 seconds
        { duration: '10s', target: 0 },  // Stage 3: Ramp down to 0 users
    ],
};

export default function () {
    // 1. The Target: Your exact API port
    const url = 'https://localhost:7116/api/Auth/login';

    // 2. The Payload: Using the Patient credentials from your earlier tests
    const payload = JSON.stringify({
        Uname: 'P-Timotei-Medi-91b1d0fe',
        Password: 'Odobesti16E'
    });

    const params = {
        headers: {
            'Content-Type': 'application/json',
        },
    };

    // 3. The Attack: Send the POST request
    const res = http.post(url, payload, params);

    // 4. The Validation: Ensure the server isn't buckling and returning 500s
    check(res, {
        'Login successful (200 OK)': (r) => r.status === 200,
        'Response has a token': (r) => r.body.includes('token'),
    });

    // 5. The Breath: Wait 1 second before this specific virtual user attacks again
    sleep(1);
}