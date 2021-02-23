import {check} from 'k6';
import http from 'k6/http';
export default () => {
    const params = {
    };
    const res = http.get('http://localhost:5000/materials/42', params);
    check(res, {
        'status is 200': () => res.status === 200,
    });
};