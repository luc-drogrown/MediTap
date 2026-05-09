import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    insecureSkipTLSVerify: true,

    stages: [
        { duration: '2s', target: 50 },  // RAMP UP: 0 to 50 users in just 2 seconds!
        { duration: '20s', target: 50 }, // HOLD: Hammer the server for 20 seconds
        { duration: '10s', target: 0 },  // RECOVER: Spin back down
    ],
};

export default function () {
    const url = 'https://localhost:7116/api/Auth/login';

    const payload = JSON.stringify({
        Uname: 'P-Timotei-Medi-91b1d0fe',
        Password: 'Odobesti16E'
    });

    const params = {
        headers: {
            'Content-Type': 'application/json',
        },
    };

    const res = http.post(url, payload, params);

    check(res, {
        'Login successful (200 OK)': (r) => r.status === 200,
    });

    sleep(1);
}