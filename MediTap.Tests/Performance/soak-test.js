import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    insecureSkipTLSVerify: true,
    stages: [
        { duration: '2m', target: 20 },   // RAMP UP: Slowly build to 20 users over 2 minutes
        { duration: '15m', target: 20 },  // SOAK: Hold them there for 15 minutes
        { duration: '2m', target: 0 },    // RAMP DOWN: Cool off
    ],
};

// Setup: Log in once and get the token
export function setup() {
    const loginRes = http.post('https://localhost:7116/api/Auth/login', JSON.stringify({
        Uname: 'P-Timotei-Medi-91b1d0fe',
        Password: 'Odobesti16E'
    }), { headers: { 'Content-Type': 'application/json' } });

    return { token: loginRes.json('token') };
}

// The Swarm: Continually fetch data
export default function (data) {
    const url = 'https://localhost:7116/api/Patient';

    const params = {
        headers: {
            'Authorization': `Bearer ${data.token}`,
            'Content-Type': 'application/json',
        },
    };

    const res = http.get(url, params);

    check(res, {
        'Status is 200': (r) => r.status === 200,
    });

    sleep(1);
}