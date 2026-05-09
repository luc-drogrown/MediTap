import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {

    insecureSkipTLSVerify: true,

    // Ramp up, hold the pressure, ramp down
    stages: [
        { duration: '10s', target: 20 },
        { duration: '30s', target: 20 },
        { duration: '10s', target: 0 },
    ],
};

// --- SETUP PHASE ---
// This runs exactly ONCE before the swarm begins.
// We use it to log in and steal the JWT token.
export function setup() {
    const loginUrl = 'https://localhost:7116/api/Auth/login';
    const payload = JSON.stringify({
        Uname: 'P-Timotei-Medi-91b1d0fe',
        Password: 'Odobesti16E'
    });
    const params = { headers: { 'Content-Type': 'application/json' } };

    const loginRes = http.post(loginUrl, payload, params);

    // If login fails, the test will crash here (which is what we want!)
    if (loginRes.status !== 200) {
        throw new Error(`Login failed during setup! Status: ${loginRes.status}`);
    }

    // Extract the token and pass it to the Virtual Users
    return { token: loginRes.json('token') };
}


// --- THE SWARM ---
// This runs repeatedly for every Virtual User.
// Notice the 'data' parameter—that contains the token from the setup phase!
export default function (data) {

    // CHANGE THIS if your fetch endpoint is different!
    const url = 'https://localhost:7116/api/Patient';

    const params = {
        headers: {
            'Authorization': `Bearer ${data.token}`, // Attaching the VIP pass
            'Content-Type': 'application/json',
        },
    };

    // The Attack: Send the GET request
    const res = http.get(url, params);

    // ADD THIS LINE: If it fails, print the status code to the terminal!
    if (res.status !== 200) {
        console.log(`Failed! Status: ${res.status}`);
    }

    // The Validation
    check(res, {
        'Fetch successful (200 OK)': (r) => r.status === 200,
    });

    // Wait 1 second before fetching again
    sleep(1);
}