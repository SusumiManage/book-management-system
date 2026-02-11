import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';

import { AuthService } from '../services/auth.service';

export const adminGuard: CanActivateFn = () => {
    const auth = inject(AuthService);
    const router = inject(Router);

    if (!auth.isLoggedIn()) {
        return router.createUrlTree(['/login']);
    }

    const role = auth.getUserRole();
    if (role !== 'Admin') {
        return router.createUrlTree(['/books']);
    }

    return true;
};
